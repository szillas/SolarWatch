

export default function SunriseSunsetResultComp({sunriseData}){

    return(
        <div>
            <p>City: {sunriseData.city}</p>
            <p>Country: {sunriseData.country}</p>
            <p>Date: {sunriseData.date}</p>
            <p> Sunrise: {sunriseData.sunrise}</p>
            <p>Sunset: {sunriseData.sunset}</p>
            <p>TimeZone: {sunriseData.timeZone}</p>
            <p>{}</p>
        </div>
    )
}
