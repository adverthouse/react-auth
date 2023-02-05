import React, { useState } from 'react'
import { useHistory } from 'react-router-dom';
import axios from 'axios';
import { useToken } from '../auth/useToken';

const LoginPage = () => {

    const [, setToken] = useToken();

    const [errorMessage, ] = useState("")
    const [emailValue, setEmail] = useState("");
    const [passwordValue, setPassword] = useState("");
    
    const history = useHistory();
    
    const onLoginClick = async () => {
       const response = await axios.post('https://localhost:7177/api/v1/auth/authenticate',{
          username: emailValue,
          password: passwordValue
       });

       const { jwtToken } = response.data; 
       setToken(jwtToken);
       history.push("/");
    }

    return (  
        <div className="content-container">
        <h1>Log in</h1>
        {errorMessage && <div className='fail'>{errorMessage}</div>}
        <input
            value={emailValue}
            onChange={e=> setEmail(e.target.value)}
            placeholder="email@email.com" />
        <input 
            value={passwordValue}
               type="password" placeholder="password" 
               onChange={e=> setPassword(e.target.value)}/>
        <button disabled={!emailValue || !passwordValue}
         onClick={onLoginClick}>Log in</button>
        <button onClick={() => history.push("/forgot-password")}>Forgot my password</button>
        <button onClick={() => history.push("/signup")}>Sign up</button>
   </div>
    );
}
 
export default LoginPage;