<template>
  <VcBlade
    :title="item.permalink ?? ''"
    :toolbar-items="bladeToolbar"
    :loading="loading"
    width="40%"
  >
    <VcContainer>
      <VcForm class="seo-form tw-space-y-4">
        <VcField
          :label="$t('SEO.FIELDS.PERMALINK')"
          :model-value="item.permalink"
          copyable
        />
        <VcSelect
          v-model="item.status"
          :label="$t('SEO.FIELDS.STATUS')"
          :options="statuses"
          :clearable="false"
        />
        <VcInput
          v-model="item.redirectUrl"
          :label="$t('SEO.FIELDS.REDIRECT_URL')"
          :required="item.status === 'Resolved'"
        />
        <VcRow class="tw-gap-4">
          <VcCol>
            <VcField
              :label="$t('SEO.FIELDS.HITS')"
              :model-value="item.hitCount"
            />
          </VcCol>
          <VcCol>
            <VcField
              :label="$t('SEO.FIELDS.LANGUAGE')"
              :model-value="item.language ?? '—'"
            />
          </VcCol>
        </VcRow>
        <VcRow class="tw-gap-4">
          <VcCol>
            <VcField
              :label="$t('SEO.FIELDS.LAST_HIT')"
              :model-value="item.lastHitDate"
              type="date"
            />
          </VcCol>
          <VcCol>
            <VcField
              :label="$t('SEO.FIELDS.CREATED')"
              :model-value="item.createdDate"
              type="date"
            />
          </VcCol>
        </VcRow>
      </VcForm>
    </VcContainer>
  </VcBlade>
</template>

<script setup lang="ts">
import { useApiClient, useAsync, useBlade, useBladeForm, useLoading, usePopup, type IBladeToolbar } from "@vc-shell/framework";
import { VcBlade, VcCol, VcContainer, VcField, VcForm, VcInput, VcRow, VcSelect } from "@vc-shell/framework/ui";
import { computed, onMounted, ref } from "vue";
import { useI18n } from "vue-i18n";
import { BrokenLinksClient, type BrokenLink } from "../../api_client/virtocommerce.seo";
import { toOptions } from "../api";

defineBlade({
  url: "/seo-broken-link",
  name: "SeoBrokenLinkDetails",
});

const { t } = useI18n({ useScope: "global" });
const { param, callParent, closeSelf } = useBlade();
const { showConfirmation } = usePopup();
const { getApiClient } = useApiClient(BrokenLinksClient);

const item = ref<BrokenLink>({});
const statuses = toOptions(["Active", "Resolved", "Accepted"]);

const form = useBladeForm({
  data: item,
  canSaveOverride: computed(() => item.value.status !== "Resolved" || !!item.value.redirectUrl),
  closeConfirmMessage: computed(() => t("SEO.ALERTS.CLOSE_CONFIRMATION")),
});

const { loading: fetching, action: load } = useAsync(async () => {
  item.value = await (await getApiClient()).getById(param.value!);
  form.setBaseline();
});

const { loading: saving, action: save } = useAsync(async () => {
  await (await getApiClient()).update(item.value);
  await load();
  await callParent("reload");
});

const loading = useLoading(fetching, saving);

const bladeToolbar = ref<IBladeToolbar[]>([
  {
    id: "save",
    title: t("SEO.TOOLBAR.SAVE"),
    icon: "lucide-save",
    permissions: "seo:update",
    disabled: computed(() => !form.canSave.value),
    clickHandler: () => save(),
  },
  {
    id: "reset",
    title: t("SEO.TOOLBAR.RESET"),
    icon: "lucide-undo-2",
    disabled: computed(() => !form.isModified.value),
    clickHandler: () => form.revert(),
  },
  {
    id: "delete",
    title: t("SEO.TOOLBAR.DELETE"),
    icon: "lucide-trash-2",
    permissions: "seo:delete",
    async clickHandler() {
      if (await showConfirmation(t("SEO.ALERTS.DELETE_SELECTED", { count: 1 }))) {
        await (await getApiClient()).delete([item.value.id!]);
        form.setBaseline();
        await callParent("reload");
        await closeSelf();
      }
    },
  },
]);

onMounted(load);
</script>
