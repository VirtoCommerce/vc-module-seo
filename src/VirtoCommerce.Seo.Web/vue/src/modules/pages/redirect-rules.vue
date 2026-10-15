<template>
  <VcBlade
    :title="$t('SEO.WIDGETS.REDIRECT_RULES')"
    :toolbar-items="bladeToolbar"
    :loading="loading"
    width="50%"
  >
    <VcDataTable
      v-model:search-value="keyword"
      v-model:selection="selection"
      selection-mode="multiple"
      :items="items"
      :total-count="totalCount"
      :pagination="pagination"
      :searchable="true"
      :state-key="'STORE_REDIRECT_RULES'"
      :active-item-id="activeChildParam"
      :empty-state="{ icon: 'lucide-milestone', title: $t('SEO.NO_RULES') }"
      @row-click="(e: { data: RedirectRule }) => openDetails(e.data.id)"
      @pagination-click="pagination.goToPage"
    >
      <VcColumn
        id="inbound"
        :title="$t('SEO.FIELDS.INBOUND')"
        always-visible
      />
      <VcColumn
        id="outbound"
        :title="$t('SEO.FIELDS.OUTBOUND')"
      />
      <VcColumn
        id="redirectRuleType"
        :title="$t('SEO.FIELDS.RULE_TYPE')"
        width="100px"
      />
      <VcColumn
        id="isActive"
        :title="$t('SEO.FIELDS.STATUS')"
      >
        <template #body="{ data: rule }">
          <div class="tw-inline-flex">
            <VcStatus :variant="rule.isActive ? 'success' : 'info'">
              {{ $t(rule.isActive ? "SEO.ACTIVE" : "SEO.INACTIVE") }}
            </VcStatus>
          </div>
        </template>
      </VcColumn>
    </VcDataTable>
  </VcBlade>
</template>

<script setup lang="ts">
import { useApiClient, useAsync, useBlade, useDataTablePagination, useFunctions, usePopup, type IBladeToolbar } from "@vc-shell/framework";
import { VcBlade, VcColumn, VcDataTable, VcStatus } from "@vc-shell/framework/ui";
import { computed, onMounted, ref, watch } from "vue";
import { useI18n } from "vue-i18n";
import { RedirectRulesClient, type RedirectRule } from "../../api_client/virtocommerce.seo";

defineBlade({
  url: "/seo-redirect-rules",
  name: "SeoRedirectRules",
});

const { t } = useI18n({ useScope: "global" });
const { param, openBlade, callParent, exposeToChildren, activeChildParam } = useBlade();
const { showConfirmation } = usePopup();
const { debounce } = useFunctions();
const { getApiClient } = useApiClient(RedirectRulesClient);

const PAGE_SIZE = 20;
const items = ref<RedirectRule[]>([]);
const totalCount = ref(0);
const keyword = ref("");
const selection = ref<RedirectRule[]>([]);
const pagination = useDataTablePagination({ pageSize: PAGE_SIZE, totalCount });

const { loading, action: load } = useAsync(async () => {
  const result = await (await getApiClient()).search({ storeId: param.value, keyword: keyword.value || undefined, skip: pagination.skip, take: PAGE_SIZE });
  items.value = result.results ?? [];
  totalCount.value = result.totalCount ?? 0;
});

const bladeToolbar = ref<IBladeToolbar[]>([
  { id: "refresh", title: t("SEO.TOOLBAR.REFRESH"), icon: "lucide-refresh-cw", clickHandler: () => reload() },
  { id: "add", title: t("SEO.TOOLBAR.ADD"), icon: "lucide-plus", permissions: "seo:create", clickHandler: () => openDetails() },
  {
    id: "delete",
    title: t("SEO.TOOLBAR.DELETE"),
    icon: "lucide-trash-2",
    permissions: "seo:delete",
    disabled: computed(() => !selection.value.length),
    async clickHandler() {
      if (await showConfirmation(t("SEO.ALERTS.DELETE_SELECTED", { count: selection.value.length }))) {
        await (await getApiClient()).delete(selection.value.map((x) => x.id!));
        selection.value = [];
        await reload();
      }
    },
  },
]);

function openDetails(id?: string) {
  openBlade({ name: "SeoRedirectRuleDetails", param: id, options: { storeId: param.value } });
}

async function reload() {
  await load();
  await callParent("reloadWidgets").catch(() => undefined);
}

// from page 2 setPage(1) triggers the skip watcher, which loads
watch(keyword, debounce(() => (pagination.skip ? pagination.setPage(1) : load()), 400));
watch(() => pagination.skip, () => load());
onMounted(load);
exposeToChildren({ reload });
</script>
