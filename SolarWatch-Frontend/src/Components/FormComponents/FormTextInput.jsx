import { useState } from "react"
import './styles.css';

export default function FormTextInput({ inputNameAndId, label, setForm}){

    const[state, setState] = useState("")

    function setInput(e){
        setState(e.target.value)
        setForm(e)
    }

    return (
        <div className="control">
            <label className="label" htmlFor={inputNameAndId}>{label}</label>
            <input type="text"
                name={inputNameAndId}
                id={inputNameAndId}
                value={state}
                onChange={e => setInput(e)}
                required
                >
            </input>
        </div>
    )
}