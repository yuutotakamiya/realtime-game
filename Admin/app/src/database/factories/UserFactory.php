<?php

namespace Database\Factories;

use App\Models\User;
use Illuminate\Database\Eloquent\Factories\Factory;

/**
 * @extends \Illuminate\Database\Eloquent\Factories\Factory<\App\Models\User>
 */
class UserFactory extends Factory
{
    /**
     * Define the model's default state.
     *
     * @return array<string, mixed>
     */

    protected $model = User::class;

    public function definition(): array
    {
        return [
            'name' => $this->faker->unique()->name(),
            'level' => $this->faker->numberBetween(1, 100),
            'exp' => $this->faker->randomNumber(5),
            'life' => $this->faker->randomNumber(1),

        ];
    }
}
