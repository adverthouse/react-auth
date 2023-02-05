import  { useEffect, useState } from 'react';
import { useToken } from './useToken';


export const useUser = () => {
     const [jwtToken] = useToken();

     const getPayloadFromToken = jwtToken => {
        const encodedPayload = jwtToken.split('.')[1];

        return JSON.parse(atob(encodedPayload));
     }

     const [user, setUser] = useState(() => {
         if (!jwtToken) return null;

         return getPayloadFromToken(jwtToken);
     })

     useEffect(() => {
        if (!jwtToken)
        {
           setUser(null);
        }
        else {
             setUser(getPayloadFromToken(jwtToken));
        }
     },[]);

     return user;
}