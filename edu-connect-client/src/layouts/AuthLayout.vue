<script setup lang="ts">
interface Props {
  title?: string
  headline?: string
  description?: string
  maxWidth?: string
  align?: 'center' | 'start'
}

withDefaults(defineProps<Props>(), {
  title: 'EduConnect',
  headline: 'Empodera tu Futuro',
  description:
    'Conecta, aprende y crece en una comunidad diseñada para impulsar tu éxito académico y profesional en cada paso del camino.',
  maxWidth: 'max-w-[440px]',
  align: 'center'
})
</script>

<template>
  <main
    class="w-full min-h-screen lg:h-screen lg:overflow-hidden flex flex-col lg:flex-row bg-surface-container-lowest"
  >
    <div
      class="hidden lg:flex lg:w-[42%] lg:h-full lg:flex-none relative bg-primary overflow-hidden items-center justify-center p-12 select-none"
    >
      <div
        class="absolute top-[-10%] left-[-10%] w-[60%] h-[60%] rounded-full bg-secondary-container opacity-20 blur-[100px] pointer-events-none"
      />
      <div
        class="absolute bottom-[-15%] right-[-10%] w-[70%] h-[70%] rounded-full bg-primary-fixed-dim opacity-20 blur-[120px] pointer-events-none"
      />

      <div class="relative z-10 flex flex-col items-center text-center max-w-lg gap-8">
        <div class="flex flex-col gap-4 -mt-12">
          <h1 class="text-5xl font-bold font-display text-on-primary tracking-tight mb-2">
            {{ title }}
          </h1>
          <h2 class="text-3xl font-bold font-headline text-on-primary">
            {{ headline }}
          </h2>
          <p class="text-lg font-body text-on-primary/80 leading-relaxed">
            {{ description }}
          </p>
        </div>
      </div>
    </div>

    <div
      :class="[
        'w-full lg:w-[58%] flex-1 flex flex-col p-6 md:p-12 lg:p-16 relative bg-surface-container-lowest min-h-screen lg:min-h-0 lg:h-full lg:overflow-y-auto',
        align === 'center' ? 'items-center justify-center' : 'items-center justify-start'
      ]"
    >
      <div :class="['w-full', maxWidth]">
        <slot />
      </div>
    </div>
  </main>
</template>
