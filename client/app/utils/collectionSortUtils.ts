export const SORT_OPTIONS: { label: string; value: number }[] = [
    { label: 'Newest', value: 0 },
    { label: 'Title', value: 1 },
    { label: 'Last Practiced', value: 2 },
    { label: 'Completion', value: 3 },
    { label: 'Custom', value: 4 },
];

export const VISIBILITY_OPTIONS: {label: string; value: number}[] = [
    { label: 'Private', value: 0},
    {label: 'Friends', value: 1},
    {label: 'Public', value: 2},
]

export const reorderCollections = (newOrder: number) => {
    
}