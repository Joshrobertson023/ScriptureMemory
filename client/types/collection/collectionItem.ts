import { Note } from "../note";
import { UserPassage } from "../passages/userPassage";

export type CollectionItem =
  | { type: 'passage'; id: string; passage: UserPassage }
  | { type: 'note'; id: string; note: Note };