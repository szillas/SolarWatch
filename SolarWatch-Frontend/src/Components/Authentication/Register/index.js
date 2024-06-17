import Registration from "./Registration";
export default Registration;

export async function action({request}){
    const formData = await request.formData();
    const registrationData = Object.fromEntries(formData);

    try{
        const response = await register(registrationData);

        if(response.ok){
            return {success: true};
        } else {
            const data = await response.json();
            return { error: data.message || "Registration failed, please try again!" };
        }
    } catch (error){
        return { error:  "An unexpected error occurred. Please try again later." };
    }
}

async function register(registrationData){
    const res = await fetch("/api/Auth/Register", {
        method: "POST",
        headers: {
            "Content-Type": "application/json",
        },
        body: JSON.stringify(registrationData),
    });

    return res;
}