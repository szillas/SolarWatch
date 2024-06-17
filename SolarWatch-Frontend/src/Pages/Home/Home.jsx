import GetSunriseSunset from "../../Components/GetSunriseSunset/GetSunriseSunset"
import { AuthContext } from "../Layout/Layout"
import { useContext } from "react";
import { useNavigate } from "react-router-dom";
import "./Home.css"

export default function Home(){

    const {user} = useContext(AuthContext)|| {};
    const navigate = useNavigate()

    function navigateToSolarWatch(){
        navigate("/solar-watch")
    }

    return (
        <>
            {user &&  (
                <>
                    <div className="image-container">
                        <div className="image-box">
                            <div className="image-title">Sunrise / Sunset Times</div>
                            <img src="/images/sunset.jpg" alt="Image 1" onClick={navigateToSolarWatch} />
                        </div>
                    </div>
                </>
            )}
            {!user && <div className="login-message">Please Login to use the application!</div>}
        </> 
    )
}