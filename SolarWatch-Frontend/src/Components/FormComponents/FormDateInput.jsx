import { useState, useEffect } from "react"
import './styles.css'

export default function FormDateInput({ inputNameAndId, label, setForm}){

    const[state, setState] = useState('')

    useEffect(() => {
        // Set initial date when the component mounts
        const today = new Date();
        const formattedDate = today.toISOString().substr(0, 10); // YYYY-MM-DD format
        setState(formattedDate);
      }, []);

    function setInput(e){
        setState(e.target.value)
        setForm(e)
    }

    return (
        <div className="control">
            <label className="label" htmlFor={inputNameAndId}>{label}</label>
            <input type="date"
                name={inputNameAndId}
                id={inputNameAndId}
                value={state}
                onChange={e => setInput(e)}>
            </input>
        </div>
    )
}