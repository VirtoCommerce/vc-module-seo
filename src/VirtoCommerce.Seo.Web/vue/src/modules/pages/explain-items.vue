<template>
  <VcBlade
    :title="options?.stage ?? ''"
    width="50%"
  >
    <VcDataTable
      :items="rows"
      :state-key="'STORE_SEO_EXPLAIN_ITEMS'"
      :empty-state="{ icon: 'lucide-search-check', title: $t('SEO.EXPLAIN_EMPTY') }"
    >
      <VcColumn
        id="semanticUrl"
        :title="$t('SEO.FIELDS.SEMANTIC_URL')"
        always-visible
      />
      <VcColumn
        id="objectType"
        :title="$t('SEO.FIELDS.TYPE')"
      />
      <VcColumn
        id="objectId"
        :title="$t('SEO.FIELDS.OBJECT_ID')"
      />
      <VcColumn
        id="languageCode"
        :title="$t('SEO.FIELDS.LANGUAGE')"
      />
      <VcColumn
        id="score"
        :title="$t('SEO.FIELDS.SCORE')"
        width="80px"
      />
      <VcColumn
        id="resolved"
        :title="$t('SEO.FIELDS.RESOLVED')"
        width="100px"
      >
        <template #body="{ data: row }">
          <VcStatusIcon :status="row.resolved" />
        </template>
      </VcColumn>
      <VcColumn
        id="reasons"
        :title="$t('SEO.FIELDS.REASONS')"
      />
    </VcDataTable>
  </VcBlade>
</template>

<script setup lang="ts">
import { useBlade } from "@vc-shell/framework";
import { VcBlade, VcColumn, VcDataTable, VcStatusIcon } from "@vc-shell/framework/ui";
import { computed } from "vue";
import type { SeoExplainItem } from "../../api_client/virtocommerce.seo";

defineBlade({
  url: "/seo-explain-items",
  name: "SeoExplainItems",
});

const { options } = useBlade<{ stage: string; items: SeoExplainItem[] }>();

// one row per SEO record the resolver looked at in this stage
const rows = computed(() =>
  (options.value?.items ?? []).map((x, i) => ({
    id: x.seoInfo?.id ?? String(i),
    semanticUrl: x.seoInfo?.semanticUrl,
    objectType: x.seoInfo?.objectType,
    objectId: x.seoInfo?.objectId,
    languageCode: x.seoInfo?.languageCode,
    score: x.score,
    resolved: !!x.isResolved,
    reasons: (x.reasons ?? []).map((r) => [r.code, r.details].filter(Boolean).join(": ")).join("; "),
  })),
);
</script>
