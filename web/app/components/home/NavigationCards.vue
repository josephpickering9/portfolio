<template>
  <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6 w-full max-w-7xl">
    <NuxtLink to="/search" class="group card bg-base-100 hover:bg-base-200/50 shadow-md hover:shadow-2xl transition-all duration-300 border border-base-200 hover:border-info/50 hover:-translate-y-1">
      <div class="card-body items-center text-center p-8">
        <div class="p-4 rounded-full bg-info/10 group-hover:bg-info/20 transition-colors duration-300 mb-4">
          <Icon name="mdi:robot-happy-outline" class="!w-10 !h-10 !text-info" />
        </div>
        <h2 class="card-title text-2xl mb-2">AI Search</h2>
        <p class="text-base-content/70">Ask questions about my work experience, projects and skills</p>
      </div>
    </NuxtLink>
    <NuxtLink to="/projects" class="group card bg-base-100 hover:bg-base-200/50 shadow-md hover:shadow-2xl transition-all duration-300 border border-base-200 hover:border-primary/50 hover:-translate-y-1">
      <div class="card-body items-center text-center p-8">
        <div class="p-4 rounded-full bg-primary/10 group-hover:bg-primary/20 transition-colors duration-300 mb-4">
          <Icon name="heroicons:rectangle-stack" class="!w-10 !h-10 !text-primary" />
        </div>
        <h2 class="card-title text-2xl mb-2">Projects</h2>
        <p class="text-base-content/70">View all my projects</p>
      </div>
    </NuxtLink>
    <NuxtLink to="/companies" class="group card bg-base-100 hover:bg-base-200/50 shadow-md hover:shadow-2xl transition-all duration-300 border border-base-200 hover:border-secondary/50 hover:-translate-y-1">
      <div class="card-body items-center text-center p-8">
        <div class="p-4 rounded-full bg-secondary/10 group-hover:bg-secondary/20 transition-colors duration-300 mb-4">
          <Icon name="heroicons:building-office" class="!w-10 !h-10 !text-secondary" />
        </div>
        <h2 class="card-title text-2xl mb-2">Companies</h2>
        <p class="text-base-content/70">See companies I've worked with</p>
      </div>
    </NuxtLink>
    <NuxtLink to="/technologies" class="group card bg-base-100 hover:bg-base-200/50 shadow-md hover:shadow-2xl transition-all duration-300 border border-base-200 hover:border-accent/50 hover:-translate-y-1">
      <div class="card-body items-center text-center p-8">
        <div class="p-4 rounded-full bg-accent/10 group-hover:bg-accent/20 transition-colors duration-300 mb-4">
          <Icon name="heroicons:code-bracket" class="!w-10 !h-10 !text-accent" />
        </div>
        <h2 class="card-title text-2xl mb-2">Technologies</h2>
        <p class="text-base-content/70">Browse by technologies</p>
      </div>
    </NuxtLink>
  </div>
</template>
