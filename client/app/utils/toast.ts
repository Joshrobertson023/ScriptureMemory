import type { ToastManager, ToastShowConfig, ToastVariant } from 'heroui-native';

export interface ShowToastOptions
    extends Omit<ToastShowConfig, 'variant' | 'label' | 'description' | 'onActionPress'> {
    type?: ToastVariant;
    title?: string;
    message?: string;
    onActionPress?: (helpers: { hide: (ids?: string | string[] | 'all') => void }) => void;
}

let manager: ToastManager | null = null;

export function registerToastManager(next: ToastManager | null): void {
    manager = next;
}

export function showToast(options: ShowToastOptions | string): string | undefined {
    if (!manager) {
        console.warn('showToast called before GlobalToastBridge mounted; toast dropped:', options);
        return undefined;
    }

    if (typeof options === 'string') {
        return manager.show(options);
    }

    const { type, title, message, onActionPress, ...rest } = options;
    return manager.show({
        ...rest,
        variant: type,
        label: title,
        description: message,
        onActionPress: onActionPress ? ({ hide }) => onActionPress({ hide }) : undefined,
    });
}

export function hideToast(ids?: string | string[] | 'all'): void {
    manager?.hide(ids);
}
