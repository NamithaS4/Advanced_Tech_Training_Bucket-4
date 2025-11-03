import React, { useState, useEffect, useRef } from 'react';
import Header from '../../components/layout/Header/Header';
import EndUserSidebar from '../../components/layout/Sidebar/EndUserSidebar';

export default function ProfileSettings() {
  const [activeTab, setActiveTab] = useState('profile');
  const [profile, setProfile] = useState({
    name: '',
    email: '',
    mobile: '',
    profilePicture: '',
  });
  const [notifications, setNotifications] = useState({
    email: true,
    sms: false,
    push: true,
  });
  const [security, setSecurity] = useState({
    currentPassword: '',
    newPassword: '',
    confirmPassword: '',
  });
  const [message, setMessage] = useState('');

  const fileInputRef = useRef(null);

  useEffect(() => {
    const localData = JSON.parse(localStorage.getItem('endUserData'));
    const currentUser =
      JSON.parse(sessionStorage.getItem('mdms_current_user')) ||
      JSON.parse(localStorage.getItem('mdms_current_user'));

    if (currentUser) {
      setProfile((prev) => ({
        ...prev,
        name: currentUser.name || '',
        email: currentUser.email || '',
        mobile: currentUser.mobile || '',
        profilePicture: currentUser.profilePicture || '',
      }));
    } else if (localData) {
      setProfile((prev) => ({
        ...prev,
        name: localData.name || '',
        email: localData.email || '',
        mobile: localData.mobile || '',
        profilePicture: localData.profilePicture || '',
      }));
    }

    if (localData?.notifications) setNotifications(localData.notifications);
  }, []);

  // ✅ Update only enduser object inside mdms_auth_user array
  const handleProfileSave = () => {
    const users = JSON.parse(localStorage.getItem('mdms_auth_user')) || [];
    const currentUser =
      JSON.parse(sessionStorage.getItem('mdms_current_user')) ||
      users.find((u) => u.role === 'enduser');

    if (!currentUser) {
      setMessage('Unable to verify end user account.');
      return;
    }

    const updatedUsers = users.map((u) =>
      u.role === 'enduser'
        ? {
            ...u,
            name: profile.name || u.name,
            email: profile.email || u.email,
            mobile: profile.mobile || u.mobile,
            profilePicture: profile.profilePicture || u.profilePicture,
            notifications: notifications || u.notifications,
          }
        : u
    );

    localStorage.setItem('mdms_auth_user', JSON.stringify(updatedUsers));

    const updatedUser = updatedUsers.find((u) => u.role === 'enduser');

    localStorage.setItem('mdms_current_user', JSON.stringify(updatedUser));
    sessionStorage.setItem('mdms_current_user', JSON.stringify(updatedUser));
    localStorage.setItem('endUserData', JSON.stringify(updatedUser));

    setMessage('Profile updated successfully!');
    setTimeout(() => setMessage(''), 2500);
  };

  // ✅ Password change always updates correct enduser
  const handleSecuritySave = () => {
    const users = JSON.parse(localStorage.getItem('mdms_auth_user')) || [];
    const currentUser =
      JSON.parse(sessionStorage.getItem('mdms_current_user')) ||
      users.find((u) => u.role === 'enduser');

    if (!currentUser || currentUser.role !== 'enduser') {
      setMessage('Unable to verify end user account.');
      return;
    }

    const matched = users.find((u) => u.role === 'enduser');

    if (!matched) {
      setMessage('User not found.');
      return;
    }

    if (security.currentPassword !== matched.password) {
      setMessage('Incorrect current password!');
      return;
    }

    if (security.newPassword !== security.confirmPassword) {
      setMessage('Passwords do not match!');
      return;
    }

    const updatedUsers = users.map((u) =>
      u.role === 'enduser' ? { ...u, password: security.newPassword } : u
    );

    localStorage.setItem('mdms_auth_user', JSON.stringify(updatedUsers));

    const updatedUser = {
      ...currentUser,
      password: security.newPassword,
    };

    localStorage.setItem('mdms_current_user', JSON.stringify(updatedUser));
    sessionStorage.setItem('mdms_current_user', JSON.stringify(updatedUser));

    setMessage('Password updated successfully!');
    setSecurity({ currentPassword: '', newPassword: '', confirmPassword: '' });
    setTimeout(() => setMessage(''), 2500);
  };

  const handleNotificationSave = () => {
    const localData = JSON.parse(localStorage.getItem('endUserData')) || {};
    localData.notifications = notifications;
    localStorage.setItem('endUserData', JSON.stringify(localData));
    setMessage('Notification preferences saved!');
    setTimeout(() => setMessage(''), 2500);
  };

  const handleImageChange = (e) => {
    const file = e.target.files[0];
    if (!file) return;
    const reader = new FileReader();
    reader.onloadend = () => {
      setProfile({ ...profile, profilePicture: reader.result });
    };
    reader.readAsDataURL(file);
  };

  return (
    <div className="min-h-screen w-screen bg-gray-50 dark:bg-gray-900 text-black">
      <Header />
      <div className="flex">
        <EndUserSidebar />
        <main className="flex-1 p-6">
          <div className="max-w-4xl mx-auto">
            <h1 className="text-xl mb-6 font-semibold text-gray-900 dark:text-white">
              Profile & Settings
            </h1>

            <div className="flex border-b border-gray-300 mb-8">
              {['profile', 'security', 'notification'].map((tab) => (
                <button
                  key={tab}
                  onClick={() => setActiveTab(tab)}
                  className={`w-full text-black dark:text-white bg-transparent pb-2 text-sm font-medium relative focus:outline-none hover:border-transparent ${
                    activeTab === tab
                      ? 'after:absolute after:bottom-0 after:left-0 after:w-full after:h-[2px] after:bg-purple-500'
                      : ''
                  }`}
                >
                  {tab.charAt(0).toUpperCase() + tab.slice(1)}
                </button>
              ))}
            </div>

            {message && (
              <p className="text-center text-sm text-purple-600 mb-4">
                {message}
              </p>
            )}

            {/* --- Profile Tab --- */}
            {activeTab === 'profile' && (
              <div className="flex flex-col items-center">
                <div className="relative mb-6">
                  <div className="w-24 h-24 rounded-full bg-black flex items-center justify-center overflow-hidden">
                    {profile.profilePicture ? (
                      <img
                        src={profile.profilePicture}
                        alt="Profile"
                        className="w-full h-full object-cover"
                      />
                    ) : (
                      <i className="fas fa-user text-white text-3xl"></i>
                    )}
                  </div>
                  <button
                    onClick={() => fileInputRef.current.click()}
                    className="absolute bottom-0 right-0 bg-white border rounded-full p-1 shadow"
                  >
                    <i className="fas fa-pen text-xs text-gray-700">Edit</i>
                  </button>
                  <input
                    type="file"
                    ref={fileInputRef}
                    onChange={handleImageChange}
                    accept="image/*"
                    className="hidden"
                  />
                </div>

                <div className="w-80 space-y-5">
                  <div>
                    <label className="text-xs text-gray-600 ml-1">Name</label>
                    <input
                      type="text"
                      value={profile.name}
                      onChange={(e) =>
                        setProfile({ ...profile, name: e.target.value })
                      }
                      className="w-full border dark:bg-gray-300 dark:text-black border-gray-200 rounded px-3 py-2 bg-gray-100"
                    />
                  </div>
                  <div>
                    <label className="text-xs text-gray-600 ml-1">Email</label>
                    <input
                      type="email"
                      value={profile.email}
                      onChange={(e) =>
                        setProfile({ ...profile, email: e.target.value })
                      }
                      className="w-full dark:bg-gray-300 dark:text-black border border-gray-200 rounded px-3 py-2 bg-gray-100"
                    />
                  </div>
                  <div>
                    <label className="text-xs text-gray-600 ml-1">
                      Mobile No.
                    </label>
                    <input
                      type="text"
                      value={profile.mobile}
                      onChange={(e) =>
                        setProfile({ ...profile, mobile: e.target.value })
                      }
                      placeholder="+91-9809892782"
                      className="w-full border dark:bg-gray-300 dark:text-black border-gray-300 rounded px-3 py-2 bg-gray-100"
                    />
                  </div>
                  <button
                    onClick={handleProfileSave}
                    className="w-full bg-black dark:bg-gray-300 dark:text-black text-white py-2 rounded-full hover:bg-gray-800"
                  >
                    Save and continue
                  </button>
                </div>
              </div>
            )}

            {/* --- Security Tab --- */}
            {activeTab === 'security' && (
              <div className="flex flex-col items-center">
                <div className="w-24 h-24 rounded-full bg-black flex items-center justify-center text-white text-3xl mb-6">
                  <i className="fas fa-user"></i>
                </div>

                <div className="w-80 space-y-5">
                  <div>
                    <label className="text-xs text-gray-600 ml-1">
                      Current Password
                    </label>
                    <input
                      type="password"
                      value={security.currentPassword}
                      onChange={(e) =>
                        setSecurity({
                          ...security,
                          currentPassword: e.target.value,
                        })
                      }
                      className="w-full border dark:bg-gray-300 dark:text-black border-purple-500 rounded px-3 py-2 bg-gray-100"
                    />
                  </div>
                  <div>
                    <label className="text-xs text-gray-600 ml-1">
                      New Password
                    </label>
                    <input
                      type="password"
                      value={security.newPassword}
                      onChange={(e) =>
                        setSecurity({
                          ...security,
                          newPassword: e.target.value,
                        })
                      }
                      className="w-full border dark:bg-gray-300 dark:text-black border-gray-300 rounded px-3 py-2 bg-gray-100"
                    />
                  </div>
                  <div>
                    <label className="text-xs text-gray-600 ml-1">
                      Confirm Password
                    </label>
                    <input
                      type="password"
                      value={security.confirmPassword}
                      onChange={(e) =>
                        setSecurity({
                          ...security,
                          confirmPassword: e.target.value,
                        })
                      }
                      className="w-full border dark:bg-gray-300 dark:text-black border-gray-300 rounded px-3 py-2 bg-gray-100"
                    />
                  </div>
                  <button
                    onClick={handleSecuritySave}
                    className="w-full bg-black dark:bg-gray-300 dark:text-black text-white py-2 rounded-full hover:bg-gray-800"
                  >
                    Save and Continue
                  </button>
                </div>
              </div>
            )}

            {/* --- Notification Tab --- */}
            {activeTab === 'notification' && (
              <div className="flex flex-col items-center space-y-8">
                <h2 className="text-sm font-medium text-gray-700 dark:text-white">
                  You can get notifications from
                </h2>

                <div className="space-y-5 dark:text-white text-sm">
                  {['email', 'sms', 'push'].map((type) => (
                    <div
                      key={type}
                      className="flex items-center justify-between w-64"
                    >
                      <span className="capitalize">{type}</span>
                      <label className="relative inline-flex items-center cursor-pointer">
                        <input
                          type="checkbox"
                          checked={notifications[type]}
                          onChange={() =>
                            setNotifications({
                              ...notifications,
                              [type]: !notifications[type],
                            })
                          }
                          className="sr-only peer"
                        />
                        <div className="w-11 h-6 bg-gray-200 rounded-full peer peer-checked:bg-purple-500 after:content-[''] after:absolute after:top-[2px] after:left-[2px] after:bg-white after:h-5 after:w-5 after:rounded-full after:transition-all peer-checked:after:translate-x-full"></div>
                      </label>
                    </div>
                  ))}
                </div>

                <button
                  onClick={handleNotificationSave}
                  className="w-64 bg-black text-white dark:bg-gray-300 dark:text-black py-2 rounded-full hover:bg-gray-800"
                >
                  Save and continue
                </button>
              </div>
            )}
          </div>
        </main>
      </div>
    </div>
  );
}
