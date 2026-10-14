<template>
  <VcBlade
    :title="$t('SEO.WIDGETS.SEO_EXPLAIN')"
    :toolbar-items="bladeToolbar"
    :loading="loading"
    width="50%"
  >
    <div class="seo-inline-form">
      <VcRow class="tw-gap-4">
        <VcCol>
          <VcSelect
            v-model="languageCode"
            :label="$t('SEO.FIELDS.LANGUAGE')"
            :options="languages"
            :clearable="false"
            required
          />
        </VcCol>
        <VcCol>
          <VcSelect
            v-model="organizationId"
            :label="$t('SEO.FIELDS.ORGANIZATION')"
            :options="searchOrganizations"
            option-label="name"
            searchable
          />
        </VcCol>
      </VcRow>
      <VcInput
        v-model="permalink"
        :label="$t('SEO.FIELDS.PERMALINK')"
        placeholder="/category/product"
        required
      />
    </div>
    <VcDataTable
      :items="stages"
      :state-key="'STORE_SEO_EXPLAIN'"
      :active-item-id="activeChildParam"
      :empty-state="{ icon: 'lucide-search-check', title: $t('SEO.EXPLAIN_EMPTY') }"
      @row-click="(e: { data: Stage }) => openStage(e.data)"
    >
      <VcColumn
        id="stage"
        :title="$t('SEO.FIELDS.STAGE')"
        always-visible
      />
      <VcColumn
        id="description"
        :title="$t('SEO.FIELDS.DESCRIPTION')"
      />
      <VcColumn
        id="count"
        :title="$t('SEO.FIELDS.ITEMS')"
        width="90px"
      />
    </VcDataTable>
  </VcBlade>
</template>

<script setup lang="ts">
import { notification, useApiClient, useAsync, useBlade, type IBladeToolbar } from "@vc-shell/framework";
import { VcBlade, VcCol, VcColumn, VcDataTable, VcInput, VcRow, VcSelect } from "@vc-shell/framework/ui";
import { computed, onMounted, ref } from "vue";
import { useI18n } from "vue-i18n";
import { SeoClient, type SeoExplainResult } from "../../api_client/virtocommerce.seo";
import { api, toOptions } from "../api";

defineBlade({
  url: "/seo-explain",
  name: "SeoExplain",
});

type Stage = { id: string; stage: string; description: string; count: number; result: SeoExplainResult };

const { t, te } = useI18n({ useScope: "global" });
const { param, openBlade, activeChildParam } = useBlade();
const { getApiClient } = useApiClient(SeoClient);

const store = ref<{ defaultLanguage?: string; languages?: string[] }>({});
const languageCode = ref<string>();
const organizationId = ref<string>();
const permalink = ref("");
const results = ref<SeoExplainResult[]>([]);
const languages = computed(() => toOptions(store.value.languages));

const stages = computed<Stage[]>(() =>
  results.value.map((r) => ({
    id: String(r.stage),
    stage: String(r.stage),
    // the AngularJS blade described only some stages (in English)
    description: te(`SEO.STAGES.${r.stage}`, "en") ? t(`SEO.STAGES.${r.stage}`) : "",
    count: r.items?.length ?? 0,
    result: r,
  })),
);

async function searchOrganizations(keyword?: string, skip = 0, ids?: string[]) {
  return api.post<{ results: unknown[]; totalCount: number }>("/api/members/search", {
    memberType: "Organization",
    keyword,
    objectIds: ids,
    deepSearch: true,
    skip,
    take: ids?.length || 20,
  });
}

const { loading, action: explain } = useAsync(async () => {
  results.value = await (await getApiClient()).getExplain(
    param.value,
    organizationId.value ?? undefined, // a cleared select sets null, which the generated client rejects
    store.value.defaultLanguage,
    languageCode.value,
    permalink.value,
  );
});

const bladeToolbar = ref<IBladeToolbar[]>([
  {
    id: "explain",
    title: t("SEO.EXPLAIN"),
    icon: "lucide-play",
    permissions: "seo:read",
    disabled: computed(() => !languageCode.value || !permalink.value),
    clickHandler: () => explain(),
  },
]);

function openStage(stage: Stage) {
  openBlade({ name: "SeoExplainItems", param: stage.id, options: { stage: stage.stage, items: stage.result.items ?? [] } });
}

onMounted(async () => {
  try {
    store.value = await api.get(`/api/stores/${encodeURIComponent(param.value!)}`);
    languageCode.value = store.value.defaultLanguage;
  } catch (e) {
    notification.error(String(e));
  }
});
</script>
