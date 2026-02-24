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
    newUser: User
): Promise<void> {
    const response = await fetch(
        `${baseUrl}/users`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify(newUser),
        }
    );
    if (!response.ok) {
        const responseText = await response.text();
        throw new Error(responseText || 'Failed to create user');
    }
}

export async function loginUser(user: User): Promise<User> {
    try {
        const response = await fetch(`${baseUrl}/users/${user.username}`);
        if (response.ok) {
            const loggedInUser = (await response.json());
            try {
                loggedInUser.isAdmin = await (loggedInUser.username);
            } catch (error) {
                console.error('Failed to check admin status:', error);
            }
            return loggedInUser as User;
        } else {
            throw new Error('Login failed');
        }
    } catch (error) {
        throw error;
    }
}