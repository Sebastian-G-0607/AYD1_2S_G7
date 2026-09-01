<script setup lang="ts">
import AuthLayout from '@/layouts/AuthLayout.vue'
import { StudentRegisterForm } from '@/features/auth'
</script>

<template>
  <AuthLayout max-width="max-w-2xl" align="start">
    <StudentRegisterForm />
  </AuthLayout>
</template>
