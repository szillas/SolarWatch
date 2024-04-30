
import GetSunriseSunset from "../../Components/GetSunriseSunset/GetSunriseSunset"
import { AuthContext } from "../Layout/Layout"
import { useContext } from "react";

export default function SolarWatch(){

    const { user } = useContext(AuthContext) || {};

    return (
        <>
            {user && (
                <GetSunriseSunset />
            )}
        </>
    )
}