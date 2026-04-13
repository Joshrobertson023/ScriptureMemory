import { BibleVersion } from "../../types/enums";
import { Session, useUserStore } from "../stores/user.store";
import { useUserAuthStore } from "../stores/userAuth.store";
import { baseUrl } from "./baseUrl";

export async function createUser(session: Session): Promise<void> {
    const userStore = useUserStore.getState();
    const authStore = useUserAuthStore.getState();
    console.log('creating new account2')

    const response = await fetch(
        `${baseUrl}/users`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify(session),
        }
    );
    if (response.ok) {
        const data = await response.json();
        userStore.setUser(data.user);
        authStore.setJwt(data.jwt);
        console.info('data.user.sessions[0]:' + data.user.sessions[0])
        authStore.setSession(data.user.sessions[0]);
    } 
    if (!response.ok) {
    const errorBody = await response.text(); // use .text() not .json() in case it's not JSON
    console.error('Error body:', errorBody);
}
}

export async function getNewJwt(session: Session): Promise<void> {
    const userStore = useUserStore.getState();
    const authStore = useUserAuthStore.getState();

    const response = await fetch(
        `${baseUrl}/users/jwt`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify({session}),
        }
    );
    if (response.ok) {
        const data = await response.json();
        authStore.setJwt(data.jwt);
    } else {
        throw Error('Error getting new jwt');
    }
}

export async function loginUserWithToken(session: Session): Promise<void> {
    const userStore = useUserStore.getState();
    const authStore = useUserAuthStore.getState();

    try {
        const response = await fetch(`${baseUrl}/users/login/token`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify(session),
        });
        if (response.ok) {
            const data = await response.json();
            userStore.setUser(data.user);
            authStore.setJwt(data.jwt);
        } else {
            throw new Error('Login failed');
        }
    } catch (error) {
        throw error;
    }
}