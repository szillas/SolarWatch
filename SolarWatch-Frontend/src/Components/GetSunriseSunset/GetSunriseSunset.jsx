import { useState, useEffect } from "react"

import FormTextInput from "../FormComponents/FormTextInput"
import FormDateInput from "../FormComponents/FormDateInput"
import SunriseSunsetResultComp from "../SunriseSunsetResult/SunriseSunsetResultComp"

import { AuthContext } from "../../Pages/Layout/Layout"
import { useContext } from "react";

import './styles.css';

export default function GetSunriseSunset(){

    const {user} = useContext(AuthContext)|| {};

    const[sunriseSunset, SetSunriseSunset] = useState({
        cityName: "", date: new Date().toISOString().substr(0, 10)
    })

    const[sunriseSunsetResult, SetSunriseSunsetResult] = useState({
        city: "", country: "", date: "", sunrise: "", sunset: "", timeZone:""
    })

    function setForm(e){
        if(e.target.name === "city")
        {
            console.log(e.target.value)
            SetSunriseSunset({
                ...sunriseSunset, cityName: e.target.value
            })
        }
        if(e.target.name === "date")
        {
            console.log(e.target.value)
            SetSunriseSunset({
                ...sunriseSunset, date: e.target.value
            })
        }
    }

    async function onSubmit(e) {
        e.preventDefault();
    
        console.log(sunriseSunset);
        console.log(`/api/SunriseSunset/GetSunriseSunset?cityName=${sunriseSunset.cityName}&date=${sunriseSunset.date}`)
        console.log(user);

        try {
            const requestOptions = {
                method: 'GET',
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': `Bearer ${user.token}` // Include the JWT token in the Authorization header
                }
            };

            const response = await fetch(`/api/SunriseSunset/GetSunriseSunset?cityName=${sunriseSunset.cityName}&date=${sunriseSunset.date}`, requestOptions);
            if (!response.ok) {
                throw new Error("Error fetching data from server.")
            }
            const data = await response.json();
            console.log(data);
            SetSunriseSunsetResult({
                ...sunriseSunsetResult, city: data.city.name, country: data.city.country, date: data.date, sunrise: data.sunrise, sunset: data.sunset, timeZone: data.timeZone
            })

        } catch (error) {
            console.error(error)
            SetSunriseSunsetResult({
                ...sunriseSunsetResult, city: "", country: "", date: "", sunrise: "", sunset: "", timeZone:""
            })
        }
    }

    return(
        <div>
           <br></br>
           <div className="sunrise-sunset-outer">
            <h2>Sunrise/Sunset form</h2>
                <form className="GetSunriseSunet" onSubmit={onSubmit}>
                    <FormTextInput inputNameAndId={"city"} label={"City"} setForm={setForm}/>
                    <FormDateInput inputNameAndId={"date"} label={"Date"} setForm={setForm} today={sunriseSunset.date}/>
                    <div className="buttons">
                            <button type="submit" className="submit-button">
                            Submit
                            </button>
                        </div>
                </form>
           </div>

            {sunriseSunsetResult.city.length !== 0 && <SunriseSunsetResultComp sunriseData={sunriseSunsetResult}/>}
        </div>

    )
}