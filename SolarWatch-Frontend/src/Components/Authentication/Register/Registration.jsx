import {Form} from "react-router-dom";
import "./Registration.css";

export default function Register(){
    return (
        <div id="registration-wrapper">
            <h1>Registration</h1>
            <Form method="post" id="registration-form">
                <label>
                    <span>User name</span>
                    <input type="text" name="username" placeholder="User name"/>
                </label>
                <label>
                    <span>Email</span>
                    <input type="text" name="email" placeholder="Email"/>
                </label>
                <label>
                    <span>Password</span>
                    <input type="password" name="password" placeholder="Password"/>
                </label>
                <button type="submit">Register</button>
            </Form>
        </div>
    )
}