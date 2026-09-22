import { useEffect } from "react";
import { useToast } from "heroui-native";
import { registerToastManager } from "../utils/toast";

/**
 * Renders nothing. Mount once under `HeroUINativeProvider` (see App.tsx) so
 * `showToast`/`hideToast` (../utils/toast) have a live `ToastManager` to call
 * into from outside React - event handlers, repositories, api helpers, etc.
 */
export default function GlobalToastBridge() {
    const { toast } = useToast();

    useEffect(() => {
        registerToastManager(toast);
        return () => registerToastManager(null);
    }, [toast]);

    return null;
}
