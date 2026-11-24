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
      search-bar
      :state-key="'STORE_REDIRECT_RULES'"
      :global-filters="globalFilters"
      :active-item-id="activeChildParam"
      :empty-state="{ icon: 'lucide-milestone', title: $t('SEO.NO_RULES') }"
      @row-click="(e: { data: RedirectRule }) => openDetails(e.data.id)"
      @pagination-click="pagination.goToPage"
      @filter="onFilter"
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
import { useApiClient, useAsync, useBlade, useDataTablePagination, useFunctions, type GlobalFilterConfig, type IBladeToolbar } from "@vc-shell/framework";
import { VcBlade, VcColumn, VcDataTable, VcStatus } from "@vc-shell/framework/ui";
import { computed, onMounted, ref, watch } from "vue";
import { useI18n } from "vue-i18n";
import { useDeleteSelectedAction } from "../composables/useDeleteSelectedAction";
import { RedirectRulesClient, type RedirectRule } from "../../api_client/virtocommerce.seo";

defineBlade({
  url: "/seo-redirect-rules",
  name: "SeoRedirectRules",
});

const { t } = useI18n({ useScope: "global" });
const { param, openBlade, callParent, exposeToChildren, activeChildParam } = useBlade();
const { debounce } = useFunctions();
const { getApiClient } = useApiClient(RedirectRulesClient);

const PAGE_SIZE = 20;
const items = ref<RedirectRule[]>([]);
const totalCount = ref(0);
const keyword = ref("");
const activeOnly = ref(false);
const selection = ref<RedirectRule[]>([]);
const pagination = useDataTablePagination({ pageSize: PAGE_SIZE, totalCount });

const { loading, action: load } = useAsync(async () => {
  const result = await (await getApiClient()).search({ storeId: param.value, keyword: keyword.value || undefined, isActive: activeOnly.value, skip: pagination.skip, take: PAGE_SIZE });
  items.value = result.results ?? [];
  totalCount.value = result.totalCount ?? 0;
});

const bladeToolbar = ref<IBladeToolbar[]>([
  { id: "refresh", title: t("SEO.TOOLBAR.REFRESH"), icon: "lucide-refresh-cw", clickHandler: () => reload() },
  { id: "add", title: t("SEO.TOOLBAR.ADD"), icon: "lucide-plus", permissions: "seo:create", clickHandler: () => openDetails() },
  useDeleteSelectedAction({ selection, remove: async (ids) => (await getApiClient()).delete(ids), reload }),
]);

// the API can only narrow to active rules (isActive: true), so the filter is an "Active only" toggle
// VcDataTable types global-filters as GlobalFilterConfig[], which lacks the search bar's toggle field
const globalFilters = computed(
  () => [{ id: "activeOnly", label: t("SEO.ACTIVE_ONLY"), filter: { toggle: true } }] as unknown as GlobalFilterConfig[],
);

function onFilter(event: { filters: Record<string, unknown> }) {
  activeOnly.value = !!event.filters.activeOnly;
}

function openDetails(id?: string) {
  openBlade({ name: "SeoRedirectRuleDetails", param: id, options: { storeId: param.value } });
}

async function reload() {
  await load();
  await callParent("reloadWidgets").catch(() => undefined);
}

// from page 2 setPage(1) triggers the skip watcher, which loads
watch(keyword, debounce(() => (pagination.skip ? pagination.setPage(1) : load()), 400));
watch(activeOnly, () => (pagination.skip ? pagination.setPage(1) : load()));
watch(() => pagination.skip, () => load());
onMounted(load);
exposeToChildren({ reload });
</script>
