import Registration from "./Registration";
export default Registration;

export async function action({request}){
    const formData = await request.formData();
    const registrationData = Object.fromEntries(formData);
    console.log(registrationData);
    return await register(registrationData);
}

async function register(registrationData){
    let res = await fetch("/api/Auth/Register", {
        method: "POST",
        headers: {
            "Content-Type": "application/json",
        },
        body: JSON.stringify(registrationData),
    });
    let data = await res.json();
    console.log(data);
    return data;
}