import { Note } from "../note";
import { Passage } from "../passages/passage";

export type CollectionItem =
  | { type: 'passage'; id: number; passage: Passage }
  | { type: 'note'; id: number; note: Note };