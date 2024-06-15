import './styles.css';

export default function SunriseSunsetResultComp({sunriseData}){

    function GetDateFromString(string){
        let indexOfT = string.indexOf('T');
        console.log(indexOfT);
        let date = string.slice(0, indexOfT);
        return date;
    }

    return(
        <div className="results-outer">
            <h1>Results</h1>
            <div className="result-item">
                <span className="result-label">City:</span>
                <span className="result-value">{sunriseData.city}</span>
            </div>
            <div className="result-item">
                <span className="result-label">Country:</span>
                <span className="result-value">{sunriseData.country}</span>
            </div>
            <div className="result-item">
                <span className="result-label">Date:</span>
                <span className="result-value">{GetDateFromString(sunriseData.date)}</span>
            </div>
            <div className="result-item">
                <span className="result-label">Sunrise:</span>
                <span className="result-value">{sunriseData.sunrise}</span>
            </div>
            <div className="result-item">
                <span className="result-label">Sunset:</span>
                <span className="result-value">{sunriseData.sunset}</span>
            </div>
            <div className="result-item">
                <span className="result-label">TimeZone:</span>
                <span className="result-value">{sunriseData.timeZone}</span>
            </div>
        </div>
    )
}
