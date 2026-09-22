import { BottomSheet, ListGroup, Separator } from 'heroui-native';
import { Archive, ArchiveRestore, Pencil, Trash } from 'lucide-react-native';
import React from 'react';
import { Alert, View } from 'react-native';
import { archiveCollection, deleteCollection, unarchiveCollection } from '../../database/repositories/collections.repository';
import { pushEditCollectionRoute } from '../../navigation';
import { useBottomSheetsStore } from '../../stores/bottomSheets.store';
import useAppTheme from '../../theme';

const DELETE_COLOR = '#E25D5D';

const CollectionMenuBottomSheet = () => {
    const theme = useAppTheme();
    const {
        collectionMenuBottomSheet: collection,
        collectionMenuSheetOpen,
        setCollectionMenuSheetOpen,
    } = useBottomSheetsStore();

    const close = () => setCollectionMenuSheetOpen(false);

    return (
        <BottomSheet isOpen={collectionMenuSheetOpen} onOpenChange={setCollectionMenuSheetOpen}>
            <BottomSheet.Portal disableFullWindowOverlay>
                <BottomSheet.Overlay />
                <BottomSheet.Content>
                    <View className="flex-row items-center justify-between px-5 pb-2">
                        <BottomSheet.Title>{collection?.title}</BottomSheet.Title>
                        <BottomSheet.Close />
                    </View>

                    <ListGroup className="mx-4 mb-6">
                        <ListGroup.Item
                            onPress={() => {
                                close();
                                if (collection) pushEditCollectionRoute(collection.id);
                            }}
                        >
                            <ListGroup.ItemPrefix>
                                <Pencil size={20} color={theme.colors.onBackground} />
                            </ListGroup.ItemPrefix>
                            <ListGroup.ItemContent>
                                <ListGroup.ItemTitle>Edit</ListGroup.ItemTitle>
                            </ListGroup.ItemContent>
                        </ListGroup.Item>
                        <Separator className="mx-4" />
                        <ListGroup.Item
                            onPress={() => {
                                close();
                                if (!collection) return;
                                if (collection.isArchived) {
                                    unarchiveCollection(collection.id);
                                } else {
                                    archiveCollection(collection.id);
                                }
                            }}
                        >
                            <ListGroup.ItemPrefix>
                                {collection?.isArchived ? (
                                    <ArchiveRestore size={20} color={theme.colors.onBackground} />
                                ) : (
                                    <Archive size={20} color={theme.colors.onBackground} />
                                )}
                            </ListGroup.ItemPrefix>
                            <ListGroup.ItemContent>
                                <ListGroup.ItemTitle>
                                    {collection?.isArchived ? 'Unarchive' : 'Archive'}
                                </ListGroup.ItemTitle>
                            </ListGroup.ItemContent>
                        </ListGroup.Item>
                        <Separator className="mx-4" />
                        <ListGroup.Item
                            onPress={() => {
                                close();
                                if (!collection) return;

                                Alert.alert(
                                    "Delete Collection",
                                    `Are you sure you want to delete "${collection.title}"? This cannot be undone.`,
                                    [
                                        { text: 'Cancel', style: 'cancel' },
                                        {
                                            text: 'Delete',
                                            style: 'destructive',
                                            onPress: () => deleteCollection(collection.id),
                                        },
                                    ]
                                );
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

export default CollectionMenuBottomSheet;
