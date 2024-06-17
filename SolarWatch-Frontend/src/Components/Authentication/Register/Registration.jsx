import {Form, useActionData, useNavigate} from "react-router-dom";
import { useState, useEffect } from "react";
import Modal from 'react-modal';
import "./Registration.css";

// Set the app element for accessibility
Modal.setAppElement('#root');

export default function Register(){
    const actionData = useActionData();
    const [errorMessage, setErrorMessage] = useState(null);
    const [successMessage, setSuccessMessage] = useState(null);
    const [countdown, setCountdown] = useState(5);
    const navigate = useNavigate();

    useEffect(() => {
        if (actionData) {
            if (actionData.success) {
                setSuccessMessage("Registration successful! You will be redirected to the login page.");
                const countdownInterval = setInterval(() => {
                    setCountdown((prevCount) => {
                        if (prevCount <= 1) {
                            clearInterval(countdownInterval);
                            navigate('/login');
                        }
                        return prevCount - 1;
                    });
                }, 1000);
            } else if (actionData.error) {
                setErrorMessage(actionData.error);
            }
        }
    }, [actionData, navigate]);

    const closeModal = () => {
        setErrorMessage(null);
        setSuccessMessage(null);
    };
    
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

            <Modal
                isOpen={!!errorMessage}
                onRequestClose={closeModal}
                contentLabel="Error"
                className="modal"
                overlayClassName="overlay">
                <h2>Error</h2>
                <p>{errorMessage}</p>
                <button onClick={closeModal}>Close</button>
            </Modal>

            <Modal
                isOpen={!!successMessage}
                contentLabel="Success"
                className="modal"
                overlayClassName="overlay">
                <h2>Success</h2>
                <p>{successMessage}</p>
                <p>Redirecting in {countdown} seconds...</p>
            </Modal>

        </div>
    )
}