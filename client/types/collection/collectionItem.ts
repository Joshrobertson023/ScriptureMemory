import { Note } from "../note";
import { UserPassage } from "../passages/userPassage";

export type CollectionItem =
  | { type: 'passage'; id: number; passage: UserPassage }
  | { type: 'note'; id: number; note: Note };