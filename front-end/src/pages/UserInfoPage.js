import axios from 'axios';
import { useState, useEffect } from 'react';
import { useHistory } from 'react-router-dom';
import { useToken } from '../auth/useToken';

export const UserInfoPage = () => {
    
    const [jwtToken, setToken] = useToken();
    
    const history = useHistory();
    
    const [firstName, setFirstName] = useState(''); 
 
    const [showSuccessMessage, setShowSuccessMessage] = useState(false);
    const [showErrorMessage, setShowErrorMessage] = useState(false);

    useEffect(() => {
        if (showSuccessMessage || showErrorMessage) {
            setTimeout(() => {
                setShowSuccessMessage(false);
                setShowErrorMessage(false);
            }, 3000);
        }
    }, [showSuccessMessage, showErrorMessage]);

    
    useEffect(() => { 
        const dataGet = async() => {            
            await axios.get('https://localhost:7177/api/v1/auth/',{
                headers : { Authorization : `Bearer ${jwtToken}`}
            })
            .then(response => {
                const { firstName } = response.data[0];
                setFirstName(firstName);
            });
        }

        dataGet();
    },[jwtToken]);

    
    const saveChanges = async () => {
        alert('Save functionality not implemented yet');
    }

    const logOut = () => {
        setToken(null);
        history.push("/login");
    }
    
    const resetValues = () => {
        alert('Reset functionality not implemented yet');
    }
    
    return (
        <div className="content-container">
            <h1>Info for ______ </h1>
            {showSuccessMessage && <div className="success">Successfully saved user data!</div>}
            {showErrorMessage && <div className="fail">Uh oh... something went wrong and we couldn't save changes</div>}
            <label>
                First Name:
                <input
                    onChange={e => setFirstName(e.target.value)}
                    value={firstName} />
            </label> 
            <hr />
            <button onClick={saveChanges}>Save Changes</button>
            <button onClick={resetValues}>Reset Values</button>
            <button onClick={logOut}>Log Out</button>
        </div>
    );
}