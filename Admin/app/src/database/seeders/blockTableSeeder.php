<?php

namespace Database\Seeders;

use App\Models\block;
use Illuminate\Database\Seeder;

class blockTableSeeder extends Seeder
{
    public function run(): void
    {
        block::create([
            'land_id' => 1,
            'block_user_id' => 1,
            'type' => 'aaa',
            'x_Direction' => 1,
            'z_Direction' => 1,
        ]);
    }
}
