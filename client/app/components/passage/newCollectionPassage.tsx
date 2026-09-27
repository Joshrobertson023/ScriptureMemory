import { Card } from "heroui-native";
import { GripVertical, Trash2 } from "lucide-react-native";
import React from "react";
import { StyleSheet, Text, TouchableOpacity, TouchableWithoutFeedback, View } from "react-native";
import { useIsActive, useReorderableDrag } from "react-native-reorderable-list";
import { Passage } from "../../../types/passages/passage";
import { UserPassage } from "../../../types/passages/userPassage";
import { useBibleVersion } from "../../hooks/useBibleVersion";
import useGlobalStyles from "../../styles/gobalStyles";
import useAppTheme from "../../theme";

interface NewCollectionPassageProps {
    userPassage: UserPassage;
    itemId: string;
    onRemove: (itemId: string) => void;
    isReordering?: boolean;
}

const useLocalStyles = () => StyleSheet.create({
    container: {
        marginBottom: 12
    },
    header: {
        flexDirection: 'row',
        alignItems: 'center',
        justifyContent: 'space-between',
        gap: 10,
        marginBottom: 8
    },
    headerActions: {
        flexDirection: 'row',
        alignItems: 'center',
        gap: 14
    },
    verses: {
        gap: 4
    }
})

const NewCollectionPassage = ({userPassage, itemId, onRemove, isReordering}: NewCollectionPassageProps) => {
    const passage: Passage = {
        reference: userPassage.passage.reference,
        verses: userPassage.passage.verses
    }

    const theme = useAppTheme();
    const { version: bibleVersion } = useBibleVersion();
    const globalStyles = useGlobalStyles();
    const drag = useReorderableDrag();
    const isActive = useIsActive();
    const styles = useLocalStyles();
    const displayedVersion = passage.verses !== undefined 
        ? passage.verses.at(0)?.translationContents?.at(0)?.version 
        : "";

    return (
        <TouchableWithoutFeedback onLongPress={drag} disabled={isActive}>
            <View style={styles.container}>
                <Card variant="default">
                    <Card.Header style={styles.header}>
                        <Card.Title style={globalStyles.verseReference}>
                            {passage.reference.readableReference}
                        </Card.Title>
                        <View style={styles.headerActions}>
                            {isReordering && (
                                <TouchableOpacity onPressIn={drag} disabled={isActive}>
                                    <GripVertical size={20} color={theme.colors.onBackground} />
                                </TouchableOpacity>
                            )}
                            <TouchableOpacity onPress={() => onRemove(itemId)}>
                                <Trash2 size={20} color={theme.colors.onBackground} />
                            </TouchableOpacity>
                        </View>
                    </Card.Header>
                    <Card.Body style={styles.verses}>
                        {passage.verses.map((verse) => (
                            <Text key={verse.id} style={globalStyles.verseText}>
                                {passage.verses.length > 1 && (verse.reference.verses.at(0) + ": ")}{verse.translationContents?.at(0)?.plainText}
                            </Text>
                        ))}
                        {displayedVersion && (
                            <Text style={globalStyles.verseVersionLabel}>{displayedVersion.toUpperCase()}</Text>
                        )}
                    </Card.Body>
                </Card>
            </View>
        </TouchableWithoutFeedback>
    )
}

export default NewCollectionPassage;
