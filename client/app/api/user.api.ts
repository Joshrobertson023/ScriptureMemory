import { BibleVersion } from "../../types/enums";
import { User } from "../../types/user/user";
import { baseUrl } from "./baseUrl";

export async function usernameExists(
    username: string
): Promise<boolean> {
    const response = await fetch(
        `${baseUrl}/users/exists/${encodeURIComponent(username)}`
    );
    if (!response.ok) {
        const responseText = await response.text();
        throw new Error(responseText || 'Failed to create user');
    }
    const data: { exists: boolean } = await response.json();
    return data.exists;
}

export async function createUser(
    username: string,
    firstName: string,
    lastName: string,
    email: string,
    password: string,
    bibleVersion: BibleVersion
): Promise<void> {
    const response = await fetch(
        `${baseUrl}/users`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify({username, firstName, lastName, email, password, bibleVersion}),
        }
    );
    if (!response.ok) {
        let errorMessage = 'Failed to create user';
        let errorDetails = null;
        try {
            const contentType = response.headers.get('content-type');
            if (contentType && contentType.includes('application/json')) {
                const errorJson = await response.json();
                errorMessage = errorJson.error || JSON.stringify(errorJson);
                errorDetails = errorJson.details || null;
                console.log('Full error response:', errorJson);
            } else {
                errorMessage = await response.text();
                console.log('Full error response (text):', errorMessage);
            }
        } catch (err) {
            console.log('Error parsing error response:', err);
        }
        throw new Error(errorMessage);
    }
}

export async function loginUser(username: string, password: string): Promise<User> {
    try {
        const response = await fetch(`${baseUrl}/users/login/username`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify({username, password}),
        });
        if (response.ok) {
            const loggedInUser = (await response.json());
            return loggedInUser as User;
        } else {
            throw new Error('Login failed');
        }
    } catch (error) {
        throw error;
    }
}

export async function loginUserWithToken(token: string): Promise<User> {
    try {
        const response = await fetch(`${baseUrl}/users/login/token`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify(token),
        });
        if (response.ok) {
            const loggedInUser = (await response.json());
            return loggedInUser as User;
        } else {
            throw new Error('Login failed');
        }
    } catch (error) {
        throw error;
    }
}