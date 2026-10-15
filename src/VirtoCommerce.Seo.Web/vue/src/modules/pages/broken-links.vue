<template>
  <VcBlade
    :title="$t('SEO.WIDGETS.BROKEN_LINKS')"
    :toolbar-items="bladeToolbar"
    :loading="loading"
    width="50%"
  >
    <div
      v-if="!enabled"
      class="seo-form"
    >
      <VcBanner variant="info">{{ $t("SEO.BROKEN_LINKS_DISABLED") }}</VcBanner>
    </div>
    <VcDataTable
      v-else
      v-model:search-value="keyword"
      v-model:selection="selection"
      selection-mode="multiple"
      :items="items"
      :total-count="totalCount"
      :pagination="pagination"
      :searchable="true"
      :state-key="'STORE_BROKEN_LINKS'"
      :active-item-id="activeChildParam"
      :empty-state="{ icon: 'lucide-unlink', title: $t('SEO.NO_BROKEN_LINKS') }"
      @row-click="(e: { data: BrokenLink }) => openDetails(e.data.id!)"
      @pagination-click="pagination.goToPage"
    >
      <VcColumn
        id="permalink"
        :title="$t('SEO.FIELDS.PERMALINK')"
        always-visible
      />
      <VcColumn
        id="hitCount"
        :title="$t('SEO.FIELDS.HITS')"
        width="90px"
      />
      <VcColumn
        id="lastHitDate"
        :title="$t('SEO.FIELDS.LAST_HIT')"
        type="date-ago"
      />
      <VcColumn
        id="status"
        :title="$t('SEO.FIELDS.STATUS')"
        always-visible
      >
        <template #body="{ data: link }">
          <div class="tw-inline-flex">
            <VcStatus :variant="link.status === 'Active' ? 'warning' : 'success'">{{ link.status }}</VcStatus>
          </div>
        </template>
      </VcColumn>
    </VcDataTable>
  </VcBlade>
</template>

<script setup lang="ts">
import {
  SettingClient,
  useApiClient,
  useAsync,
  useBlade,
  useDataTablePagination,
  useFunctions,
  usePopup,
  type IBladeToolbar,
} from "@vc-shell/framework";
import { VcBanner, VcBlade, VcColumn, VcDataTable, VcStatus } from "@vc-shell/framework/ui";
import { computed, onMounted, ref, watch } from "vue";
import { useI18n } from "vue-i18n";
import { BrokenLinksClient, type BrokenLink } from "../../api_client/virtocommerce.seo";

defineBlade({
  url: "/seo-broken-links",
  name: "SeoBrokenLinks",
});

const { t } = useI18n({ useScope: "global" });
const { param, openBlade, callParent, exposeToChildren, activeChildParam } = useBlade();
const { showConfirmation } = usePopup();
const { debounce } = useFunctions();
const { getApiClient } = useApiClient(BrokenLinksClient);
const { getApiClient: getSettingsClient } = useApiClient(SettingClient);

const PAGE_SIZE = 20;
const enabled = ref(true);
const items = ref<BrokenLink[]>([]);
const totalCount = ref(0);
const keyword = ref("");
const selection = ref<BrokenLink[]>([]);
const pagination = useDataTablePagination({ pageSize: PAGE_SIZE, totalCount });

const { loading, action: load } = useAsync(async () => {
  if (!enabled.value) return;
  const result = await (await getApiClient()).search({ storeId: param.value, keyword: keyword.value || undefined, skip: pagination.skip, take: PAGE_SIZE });
  items.value = result.results ?? [];
  totalCount.value = result.totalCount ?? 0;
});

const bladeToolbar = ref<IBladeToolbar[]>([
  { id: "refresh", title: t("SEO.TOOLBAR.REFRESH"), icon: "lucide-refresh-cw", clickHandler: () => reload() },
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

function openDetails(id: string) {
  openBlade({ name: "SeoBrokenLinkDetails", param: id });
}

async function reload() {
  await load();
  await callParent("reloadWidgets").catch(() => undefined);
}

// from page 2 setPage(1) triggers the skip watcher, which loads
watch(keyword, debounce(() => (pagination.skip ? pagination.setPage(1) : load()), 400));
watch(() => pagination.skip, () => load());

onMounted(async () => {
  // detection can be switched off (setting Seo.BrokenLinkDetection.Enabled)
  const setting = await (await getSettingsClient()).getGlobalSetting("Seo.BrokenLinkDetection.Enabled").catch(() => undefined);
  enabled.value = String(setting?.value ?? setting?.defaultValue ?? true) !== "false";
  await load();
});
exposeToChildren({ reload });
</script>
