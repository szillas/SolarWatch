import {Link} from "react-router-dom";
import { AuthContext } from "../../Pages/Layout/Layout";
import { useContext } from "react";
import "./MainNav.css";

export default function MainNav() {

    const {user, login, logout} = useContext(AuthContext) || {};

    function handleLogout() {
        logout();
    }
    return (
        <header>
            <div id="navigation">
                <div className="logo">
                    <Link to="/" id="nav-logo">Home</Link>
                </div>
                {user &&<ul className="menu">
                    <li>
                        <Link to="/solar-watch" id="nav-ads">Ads</Link>
                    </li>
                </ul>}
                {user === null && <div className="menu">
                    <Link to="/registration">Registration</Link>
                    <span> / </span>
                    <Link to="/login">Login</Link>
                </div>}
                {user !== null && <div className="logout">
                    <div onClick={handleLogout}>Logout</div>
                </div>}

            </div>
        </header>
    )
}