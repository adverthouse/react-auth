import React, { useState } from 'react'
import { useHistory } from 'react-router-dom';

const SingUpPage = () => {
    
    const [errorMessage, ] = useState("")
    const [emailValue, setEmail] = useState("");
    const [passwordValue, setPassword] = useState("");
    const [passwordConfirmValue, setConfirmPassword] = useState("");
    
    const history = useHistory();
    
    const onSignUpClick = async () => {
        alert("Sign up is not implemented yet");
    }

    return (  
        <div className="content-container">
        <h1>Sign up</h1>
        {errorMessage && <div className='fail'>{errorMessage}</div>}
        <input
            value={emailValue}
            onChange={e=> setEmail(e.target.value)}
            placeholder="email@email.com" />
        <input 
            value={passwordValue}
               type="password" placeholder="password" 
               onChange={e=> setPassword(e.target.value)}/>
               <input 
            value={passwordConfirmValue}
               type="password" placeholder="confim password" 
               onChange={e=> setConfirmPassword(e.target.value)}/>
        <hr/>
        <button disabled={!emailValue || !passwordValue || !passwordConfirmValue}
         onClick={onSignUpClick}>Sign up</button>
        <button onClick={() => history.push("/forgot-password")}>Forgot my password</button>
        <button onClick={() => history.push("/login")}>Already have an account? Login</button>
   </div>
    );
}
 
export default SingUpPage;