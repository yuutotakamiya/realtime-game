<?php

namespace Database\Seeders;

use App\Models\Stage;
use Illuminate\Database\Seeder;

class StageTableSeeder extends Seeder
{
    public function run(): void
    {
        Stage::create([
            'hand_num' => 100,
            'time_limit' => 60
        ]);

    }
}
