import { Trash2 } from "lucide-react-native";
import React from "react";
import { StyleProp, Text, TouchableOpacity, View, ViewStyle } from "react-native";
import { UserPassage } from "../../../types/passages/userPassage";
import { useBibleVersion } from "../../hooks/useBibleVersion";
import useGlobalStyles from "../../styles/gobalStyles";
import useAppTheme from "../../theme";

interface PassageContentProps {
    userPassage: UserPassage;
    style?: StyleProp<ViewStyle>;
    onRemove?: (itemId: string) => void;
}

const PassageContent = React.memo(({userPassage, onRemove}: PassageContentProps) => {
    const styles = useGlobalStyles();
    const { version: bibleVersion } = useBibleVersion();
    const passage = userPassage.passage;
    const theme = useAppTheme();

    if (!passage) {
        return null;
    }

    const displayedVersion = passage.verses[0]?.translationContents?.at(0)?.version;

    return (
        <View>
            <Text style={{...styles.p3, fontWeight: 600}}>{passage.reference.readableReference}</Text>
            <View>
                {passage.verses.map((verse) => (
                    <Text key={verse.id} style={styles.p3}>
                        {passage.verses.length > 1 && (verse.reference.verses.at(0) + ": ")}{verse.translationContents?.at(0)?.plainText}
                    </Text>
                ))}
            </View>
            {displayedVersion && (
                <Text style={styles.verseVersionLabel}>{displayedVersion.toUpperCase()}</Text>
            )}
            {onRemove && (
                <TouchableOpacity onPress={() => onRemove(userPassage.id || '')}>
                    <Trash2 size={20} color={theme.colors.onBackground} />
                </TouchableOpacity>
            )}
        </View>
    )
}
)

export default PassageContent;