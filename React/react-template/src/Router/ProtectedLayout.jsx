import NavBarProtected from "../Components/NavBarProtected";
import { Outlet } from "react-router-dom";
import Footer from "../Components/Footer";
import SideBar from "../Components/SideBar";

export default function ProtectedLayout() {
    return (
        <div>
            <NavBarProtected />
            <div style={{ display: 'flex', flexDirection: 'row' }}>
                <div><SideBar/></div>
                <div><Outlet/></div>
            </div>
            <Footer />
        </div>
    );
}