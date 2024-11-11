<?php

namespace Database\Seeders;

use App\Models\Landstatus;
use Illuminate\Database\Console\Seeds\WithoutModelEvents;
use Illuminate\Database\Seeder;

class LandStatusTableSeeder extends Seeder
{
    /**
     * Run the database seeds.
     */
    public function run(): void
    { //
        Landstatus::create([
            'land_id' => 1,
            'user_id' => 1,
            'land_block_num' => 100,
        ]);
    }
}
