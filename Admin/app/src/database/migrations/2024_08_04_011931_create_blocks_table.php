<?php

use Illuminate\Database\Migrations\Migration;
use Illuminate\Database\Schema\Blueprint;
use Illuminate\Support\Facades\Schema;

return new class extends Migration {
    /**
     * Run the migrations.
     */
    public function up(): void
    {
        Schema::create('blocks', function (Blueprint $table) {
            $table->id();
            $table->integer('land_id');//島のID
            $table->integer('block_user_id');//ブロックを置いたユーザーID
            $table->string('type');//ブロックの種類　
            $table->integer('x_Direction');//xの座標
            $table->integer('z_Direction');//zの座標
            $table->timestamps();
        });
    }

    /**
     * Reverse the migrations.
     */
    public function down(): void
    {
        Schema::dropIfExists('blocks');
    }
};
