<template>
  <VcBlade
    :title="item.inbound || $t('SEO.NEW_RULE')"
    :toolbar-items="bladeToolbar"
    :loading="loading"
    width="40%"
  >
    <VcContainer>
      <VcForm class="seo-form tw-space-y-4">
        <VcSwitch
          v-model="item.isActive"
          :label="$t('SEO.FIELDS.IS_ACTIVE')"
        />
        <VcRow class="tw-gap-4">
          <VcCol>
            <VcSelect
              v-model="item.redirectRuleType"
              :label="$t('SEO.FIELDS.RULE_TYPE')"
              :options="types"
              :clearable="false"
              required
            />
          </VcCol>
          <VcCol>
            <VcInput
              :model-value="item.priority ?? 0"
              :label="$t('SEO.FIELDS.PRIORITY')"
              type="number"
              required
              @update:model-value="(v) => (item.priority = Number(v) || 0)"
            />
          </VcCol>
        </VcRow>
        <VcInput
          v-model="item.inbound"
          :label="$t('SEO.FIELDS.INBOUND')"
          :error="!!inboundError"
          :error-message="inboundError"
          required
        />
        <VcInput
          v-model="item.outbound"
          :label="$t('SEO.FIELDS.OUTBOUND')"
          :error="!!outboundError"
          :error-message="outboundError"
          required
        />
      </VcForm>
    </VcContainer>
  </VcBlade>
</template>

<script setup lang="ts">
import { useApiClient, useAsync, useBlade, useBladeForm, useLoading, usePopup, type IBladeToolbar } from "@vc-shell/framework";
import { VcBlade, VcCol, VcContainer, VcForm, VcInput, VcRow, VcSelect, VcSwitch } from "@vc-shell/framework/ui";
import { computed, onMounted, ref } from "vue";
import { useI18n } from "vue-i18n";
import { RedirectRulesClient, type RedirectRule } from "../../api_client/virtocommerce.seo";
import { toOptions } from "../api";

defineBlade({
  url: "/seo-redirect-rule",
  name: "SeoRedirectRuleDetails",
});

const { t } = useI18n({ useScope: "global" });
const { param, options, callParent, closeSelf, replaceWith } = useBlade<{ storeId: string }>();
const { showConfirmation } = usePopup();
const { getApiClient } = useApiClient(RedirectRulesClient);

const isNew = computed(() => !param.value);
const item = ref<RedirectRule>({ storeId: options.value?.storeId, isActive: true, priority: 0, redirectRuleType: "Static" as RedirectRule["redirectRuleType"] });
const types = toOptions(["Static", "Regex"]);

// the AngularJS rules: a regex inbound must compile, and the outbound may only use its capture groups ($1..$n)
const groups = computed(() => {
  try {
    return new RegExp(`${item.value.inbound}|`).exec("")!.length - 1;
  } catch {
    return -1;
  }
});
const isRegex = computed(() => item.value.redirectRuleType === "Regex");
const inboundError = computed(() => (isRegex.value && item.value.inbound && groups.value < 0 ? t("SEO.ERRORS.REGEX") : ""));
const outboundError = computed(() => {
  if (!isRegex.value || !item.value.outbound) return "";
  const used = Math.max(0, ...[...item.value.outbound.matchAll(/\$(\d+)/g)].map((m) => Number(m[1])));
  return used > groups.value ? t("SEO.ERRORS.GROUPS", { count: Math.max(groups.value, 0) }) : "";
});

const form = useBladeForm({
  data: item,
  canSaveOverride: computed(() => !!item.value.inbound && !!item.value.outbound && !inboundError.value && !outboundError.value),
  closeConfirmMessage: computed(() => t("SEO.ALERTS.CLOSE_CONFIRMATION")),
});

const { loading: fetching, action: load } = useAsync(async () => {
  item.value = await (await getApiClient()).getById(param.value!);
  form.setBaseline();
});

const { loading: saving, action: save } = useAsync(async () => {
  const client = await getApiClient();
  if (isNew.value) {
    const created = await client.create(item.value);
    form.setBaseline();
    await callParent("reload");
    await replaceWith({ name: "SeoRedirectRuleDetails", param: created.id, options: options.value });
  } else {
    await client.update(item.value);
    await load();
    await callParent("reload");
  }
});

const loading = useLoading(fetching, saving);

const bladeToolbar = ref<IBladeToolbar[]>([
  {
    id: "save",
    title: computed(() => t(isNew.value ? "SEO.TOOLBAR.CREATE" : "SEO.TOOLBAR.SAVE")),
    icon: "lucide-save",
    permissions: isNew.value ? "seo:create" : "seo:update",
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
    isVisible: computed(() => !isNew.value),
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

onMounted(async () => {
  if (!isNew.value) await load();
  else form.setBaseline();
});
</script>
