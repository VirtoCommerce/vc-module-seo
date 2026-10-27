<template>
  <VcWidget
    :icon="icon"
    :title="$t(title)"
    :value="count"
    :disabled="!storeId"
    @click="open"
  />
</template>

<script setup lang="ts">
import { computed, ref, watch } from "vue";
import { injectBladeContext, useBlade, useWidgetTrigger } from "@vc-shell/framework";
import { VcWidget } from "@vc-shell/framework/ui";

// One of the SEO module's widgets on the store blade; the store comes from defineBladeContext({ item })
const props = defineProps<{
  icon: string;
  title: string;
  blade: string;
  // the badge: a search of this module counted for the store (totalCount)
  count?: (storeId: string) => Promise<number | undefined>;
}>();

const ctx = injectBladeContext();
const storeId = computed(() => (ctx.value.item as { id?: string } | undefined)?.id);
const { openBlade } = useBlade();
const count = ref<number | string>();

async function load() {
  if (!storeId.value || !props.count) return;
  count.value = (await props.count(storeId.value).catch(() => undefined)) ?? "";
}
watch(storeId, load, { immediate: true });
useWidgetTrigger({ onRefresh: load });

const open = () => openBlade({ name: props.blade, param: storeId.value });
</script>
