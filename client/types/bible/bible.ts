// Mirrors ScriptureMemory.Server.DataAccess.Models.Bible - excluding AbbreviationLocal
// ([Obsolete] server-side), and Authorized/LastSync/NextScheduledAutoSync (server-internal
// sync bookkeeping the client has no use for).
export interface Bible {
    id: string;
    abbreviation: string;
    name: string;
    nameLocal: string | null;
    copyright: string | null;
    info: string;
    active: boolean;
}
