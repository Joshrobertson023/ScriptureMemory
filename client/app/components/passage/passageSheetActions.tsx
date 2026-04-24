import { Text, TouchableOpacity, View } from "react-native";
import { BookmarkPlus, BookOpenText, Brain, NotebookText, Share2 } from "lucide-react-native";
import useGlobalStyles from "../../styles/gobalStyles";
import useAppTheme from "../../theme";
import { UserPassage } from "../../../types/passages/userPassage";
import { useBottomSheetsStore } from "../../stores/bottomSheets.store";

interface PassageSheetActionsProps {
    passageBottomSheet: UserPassage;
}

const PassageSheetActions = ({ passageBottomSheet }: PassageSheetActionsProps) => {
    const globalStyles = useGlobalStyles();
    const theme = useAppTheme();
    const {
        setSaveToCollectionBottomSheet,
        setSaveToCollectionSheetOpen,
        setViewNotesBottomSheet,
        setViewNotesSheetOpen
    } = useBottomSheetsStore();

    return (
        <View style={{ display: 'flex', flexDirection: 'row', justifyContent: 'center', alignItems: 'center', gap: 5 }}>
            <TouchableOpacity
                style={globalStyles.elevationButtomSquare}
                onPress={() => {
                    setSaveToCollectionBottomSheet(passageBottomSheet);
                    setSaveToCollectionSheetOpen(true);
                }}
            >
                <BookmarkPlus size={20} color={theme.colors.onBackground} strokeWidth={1.3} />
                <Text style={{ ...globalStyles.p4, fontWeight: 600 }}>
                    Save
                </Text>
            </TouchableOpacity>
            <TouchableOpacity
                style={globalStyles.elevationButtomSquare}
                onPress={() => {
                    setViewNotesBottomSheet(passageBottomSheet);
                    setViewNotesSheetOpen(true);
                }}
            >
                <NotebookText size={20} color={theme.colors.onBackground} strokeWidth={1.3} />
                <Text style={{ ...globalStyles.p4, fontWeight: 600 }}>
                    Notes
                </Text>
            </TouchableOpacity>
            <TouchableOpacity style={globalStyles.elevationButtomSquare}>
                <Brain size={20} color={theme.colors.onBackground} strokeWidth={1.3} />
                <Text style={{ ...globalStyles.p4, fontWeight: 600 }}>
                    Practice
                </Text>
            </TouchableOpacity>
            <TouchableOpacity style={globalStyles.elevationButtomSquare}>
                <BookOpenText size={20} color={theme.colors.onBackground} strokeWidth={1.3} />
                <Text style={{ ...globalStyles.p4, fontWeight: 600 }}>
                    Read
                </Text>
            </TouchableOpacity>
            <TouchableOpacity style={globalStyles.elevationButtomSquare}>
                <Share2 size={20} color={theme.colors.onBackground} strokeWidth={1.3} />
                <Text style={{ ...globalStyles.p4, fontWeight: 600 }}>
                    Share
                </Text>
            </TouchableOpacity>
        </View>
    );
};

export default PassageSheetActions;