import { BottomSheet, ListGroup } from 'heroui-native';
import { Trash } from 'lucide-react-native';
import React from 'react';
import { View } from 'react-native';
import { removeItemFromCollection } from '../../database/repositories/collections.repository';
import { useBottomSheetsStore } from '../../stores/bottomSheets.store';

const DELETE_COLOR = '#E25D5D';

const PassageMenuBottomSheet = () => {
    const {
        passageMenuBottomSheet: item,
        passageMenuSheetOpen,
        setPassageMenuSheetOpen,
    } = useBottomSheetsStore();

    const close = () => setPassageMenuSheetOpen(false);

    return (
        <BottomSheet isOpen={passageMenuSheetOpen} onOpenChange={setPassageMenuSheetOpen}>
            <BottomSheet.Portal disableFullWindowOverlay>
                <BottomSheet.Overlay />
                <BottomSheet.Content>
                    <View className="flex-row items-center justify-between px-5 pb-2">
                        <BottomSheet.Title>{item?.userPassage.passage.reference.readableReference}</BottomSheet.Title>
                        <BottomSheet.Close />
                    </View>

                    <ListGroup className="mx-4 mb-6">
                        <ListGroup.Item
                            onPress={() => {
                                close();
                                if (item) removeItemFromCollection(item.collectionId, item.itemId);
                            }}
                        >
                            <ListGroup.ItemPrefix>
                                <Trash size={20} color={DELETE_COLOR} />
                            </ListGroup.ItemPrefix>
                            <ListGroup.ItemContent>
                                <ListGroup.ItemTitle style={{ color: DELETE_COLOR }}>Delete</ListGroup.ItemTitle>
                            </ListGroup.ItemContent>
                        </ListGroup.Item>
                    </ListGroup>
                </BottomSheet.Content>
            </BottomSheet.Portal>
        </BottomSheet>
    );
};

export default PassageMenuBottomSheet;
