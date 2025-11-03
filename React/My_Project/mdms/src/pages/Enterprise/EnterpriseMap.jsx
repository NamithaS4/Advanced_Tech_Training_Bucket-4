import React, { useEffect } from 'react';
import { MapContainer, TileLayer, Marker, Popup, useMap } from 'react-leaflet';
import L from 'leaflet';
import { MapPin, AlertTriangle, Activity } from 'lucide-react';
import 'leaflet/dist/leaflet.css';

const redDotIcon = new L.Icon({
  iconUrl: 'https://cdn-icons-png.flaticon.com/512/684/684908.png',
  iconSize: [25, 25],
  iconAnchor: [12, 24],
  popupAnchor: [0, -20],
});

function FixMap() {
  const map = useMap();
  useEffect(() => {
    setTimeout(() => {
      map.invalidateSize();
    }, 400);
  }, [map]);
  return null;
}

export default function EnterpriseMap({ zones }) {
  return (
    <div className="col-span-2 bg-white dark:bg-gray-800 border border-gray-200 dark:border-gray-700 p-6 rounded-lg shadow-sm">
      <div className="relative h-[450px] w-full rounded-xl overflow-hidden">
        <MapContainer
          center={[20.5937, 78.9629]}
          zoom={5}
          scrollWheelZoom={true}
          className="h-full w-full rounded-xl z-0"
        >
          <FixMap />

          <TileLayer
            url="https://{s}.basemaps.cartocdn.com/dark_all/{z}/{x}/{y}{r}.png"
            attribution="&copy; OpenStreetMap contributors"
          />

          {zones.map((zone) => (
            <Marker
              key={zone.id}
              position={[zone.lat, zone.lng]}
              icon={redDotIcon}
            >
              <Popup>
                <div className="text-sm text-gray-800 dark:text-white">
                  <p className="font-semibold flex items-center gap-1">
                    <MapPin size={12} /> {zone.name} zone
                  </p>
                  <div className="flex items-center justify-between mt-2 text-xs">
                    <span className="flex items-center gap-1">
                      <Activity size={10} /> {zone.meters} meters
                    </span>
                    <span className="flex items-center gap-1 text-red-600">
                      <AlertTriangle size={10} /> {zone.alerts}
                    </span>
                  </div>
                </div>
              </Popup>
            </Marker>
          ))}
        </MapContainer>
      </div>
    </div>
  );
}
