import { Status } from "../enums";
import { Paid } from "./paid";
import { UserSettings } from "./userSettings";

export interface User {
    id: number;
    username: string;
    firstName: string;
    lastName: string;
    email: string;
    hashedPassword?: string;
    authToken?: string;
    status: Status;
    dateRegistered: Date;
    lastSeen: Date;
    settings: UserSettings;
    profileDescription?: string;
    pushNotificationToken?: string;
    profilePictureUrl?: string;
    versesMemorizedCount: number;
    isAdmin: boolean;
    points: number;
    paid: Paid;
    collectionsCount: number;
}