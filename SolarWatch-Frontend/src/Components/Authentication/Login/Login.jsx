import { useState, useContext } from "react"
import { AuthContext } from "../../../Pages/Layout/Layout"
import { useNavigate } from "react-router-dom";
import "./Login.css"

export default function Login() {

    const [email, setEmail] = useState(null);
    const [password, setPassword] = useState(null);
    const [isLoginSuccessful, setIsLoginSuccessful] = useState(null);
    const { login } = useContext(AuthContext) || {};
    const navigate = useNavigate();

    async function handleLogin(e) {
        e.preventDefault();
        const userToLogin = {
            email: email,
            password: password
        };
        setIsLoginSuccessful(await login(userToLogin));
        navigate("/")
    }

    return (
        <div id="login">
            <br></br>
            <h1>Login</h1>
            <form id="login-form" onSubmit={handleLogin}>
                <label>
                    <span>Email</span>
                    <input type="text" name="email" placeholder="Email" onChange={(e) => setEmail(e.target.value)}/>
                </label>
                <label>
                    <span>Password</span>
                    <input type="password" name="password" placeholder="Password"
                               onChange={(e) => setPassword(e.target.value)}/>
                </label>
                <button type="submit">Login</button>
            </form>
            { isLoginSuccessful === false && (
                    <div id="invalid-login">
                        <p>Login is not successful, try again!</p>
                    </div>
                )}
        </div>
    )
}