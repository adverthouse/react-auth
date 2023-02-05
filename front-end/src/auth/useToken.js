import { useState } from "react";

export const useToken = () => {

    const [jwtToken, setTokenInternal] = useState(()=> {
        return localStorage.getItem("jwtToken");
    });

    const setToken = newToken  => {
        if (newToken == null){
            
           localStorage.removeItem("jwtToken");
        }
        else {
            localStorage.setItem("jwtToken",newToken);
            setTokenInternal(newToken)
        }          
    };

    return [jwtToken, setToken ];    
}
 
