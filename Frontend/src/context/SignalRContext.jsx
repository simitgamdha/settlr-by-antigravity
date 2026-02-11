import { createContext, useContext, useEffect, useState } from 'react';
import * as signalR from '@microsoft/signalr';
import { authService } from '../services/authService';

const SignalRContext = createContext();

export const useSignalR = () => useContext(SignalRContext);

export const SignalRProvider = ({ children }) => {
    const [connection, setConnection] = useState(null);

    useEffect(() => {
        const token = localStorage.getItem('token');
        if (!token) return;

        // Build the connection
        const newConnection = new signalR.HubConnectionBuilder()
            .withUrl("http://localhost:5028/hubs/settlr", {
                accessTokenFactory: () => token
            })
            .withAutomaticReconnect()
            .build();

        setConnection(newConnection);
    }, []);

    useEffect(() => {
        if (connection) {
            connection.start()
                .then(() => console.log('SignalR Connected!'))
                .catch(err => console.error('SignalR Connection Error: ', err));

            return () => {
                connection.stop();
            };
        }
    }, [connection]);

    const joinGroup = async (groupId) => {
        if (connection && connection.state === signalR.HubConnectionState.Connected) {
            try {
                await connection.invoke("JoinGroup", groupId.toString());
                console.log(`Joined group ${groupId}`);
            } catch (err) {
                console.error("SignalR JoinGroup Error: ", err);
            }
        }
    };

    return (
        <SignalRContext.Provider value={{ connection, joinGroup }}>
            {children}
        </SignalRContext.Provider>
    );
};
