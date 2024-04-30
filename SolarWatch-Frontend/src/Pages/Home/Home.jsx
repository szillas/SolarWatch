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
                    <div>
                        <p>Check out Solar Watch!</p>
                        <button onClick={navigateToSolarWatch}>Solar Watch</button>
                    </div>
                )}
                {!user && <div>Please Login to use the application!</div>}
        </> 
    )
}