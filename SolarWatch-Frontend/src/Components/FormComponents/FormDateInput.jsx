import { useState, useEffect } from "react"
import './styles.css'

export default function FormDateInput({ inputNameAndId, label, setForm, today}){

    const[state, setState] = useState(today)

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