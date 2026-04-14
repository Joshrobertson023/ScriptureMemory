import { Text } from "react-native";

interface SearchResultsProps {
    query: string;
}

const SearchResult = ({query}: SearchResultsProps) => {
    return (
        <Text>{query}</Text>
    )
}

export default SearchResult;