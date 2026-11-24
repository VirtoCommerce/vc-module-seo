import { usePopup, type IBladeToolbar } from "@vc-shell/framework";
import { computed, type Ref } from "vue";
import { useI18n } from "vue-i18n";

interface Options<T extends { id?: string }> {
  selection: Ref<T[]>;
  remove: (ids: string[]) => Promise<unknown>;
  reload: () => Promise<unknown>;
}

/** Toolbar "Delete" for a list blade: confirms, deletes the selected rows, clears the selection and reloads. */
export function useDeleteSelectedAction<T extends { id?: string }>({ selection, remove, reload }: Options<T>): IBladeToolbar {
  const { t } = useI18n({ useScope: "global" });
  const { showConfirmation } = usePopup();

  return {
    id: "delete",
    title: t("SEO.TOOLBAR.DELETE"),
    icon: "lucide-trash-2",
    permissions: "seo:delete",
    disabled: computed(() => !selection.value.length),
    async clickHandler() {
      if (await showConfirmation(t("SEO.ALERTS.DELETE_SELECTED", { count: selection.value.length }))) {
        await remove(selection.value.map((x) => x.id!));
        selection.value = [];
        await reload();
      }
    },
  };
}
